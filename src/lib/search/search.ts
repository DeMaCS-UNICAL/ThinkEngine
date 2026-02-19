// src/lib/search/search.ts
import MiniSearch from 'minisearch';

export type Doc = {
  id: number;
  path: string;        // /docs/.../
  title: string;
  description: string;
  body: string;        // plain text
};

let _docs: Doc[] | null = null;
let _mini: MiniSearch<Doc> | null = null;

/**
 * Vite 5/6+: usa "query" e "import".
 * Usiamo eager:true per evitare lazy loading in client.
 */
const rawModules = import.meta.glob('/src/content/**/*.md', {
  query: '?raw',
  import: 'default',
  eager: true
});

function toRoutePath(filePath: string): string {
  let p = filePath
    .replace('/src/content', '/docs')
    .replace(/\/index\.md$/, '/')
    .replace(/\.md$/, '/');
  p = p.replace(/\/{2,}/g, '/');
  if (!p.endsWith('/')) p += '/';
  return p;
}

/** frontmatter parser minimalista, browser-safe */
function parseFrontmatter(raw: string): { data: Record<string, string>, content: string } {
  // frontmatter solo se il file INIZIA con ---
  if (!raw.startsWith('---')) {
    return { data: {}, content: raw };
  }
  const end = raw.indexOf('\n---', 3); // cerca la chiusura
  if (end === -1) {
    return { data: {}, content: raw };
  }

  const fmBlock = raw.slice(3, end).trim();      // senza i ---
  const rest = raw.slice(end + 4).replace(/^\r?\n/, ''); // contenuto dopo il frontmatter

  const data: Record<string, string> = {};
  // parsiamo righe tipo: key: value
  for (const line of fmBlock.split(/\r?\n/)) {
    const m = line.match(/^([A-Za-z0-9_-]+)\s*:\s*(.*)$/);
    if (!m) continue;
    const key = m[1].trim();
    let value = m[2].trim();
    // rimuovi virgolette se presenti all'inizio/fine
    if ((value.startsWith('"') && value.endsWith('"')) || (value.startsWith("'") && value.endsWith("'"))) {
      value = value.slice(1, -1);
    }
    data[key] = value;
  }

  return { data, content: rest };
}

/** pulizia del markdown per l’indice di ricerca */
function stripMarkdown(md: string): string {
  return md
    .replace(/```[\s\S]*?```/g, ' ')                    // code blocks
    .replace(/`[^`]+`/g, ' ')                           // inline code
    .replace(/!\[[^\]]*]\([^)]*\)/g, ' ')               // images
    .replace(/\[[^\]]*]\([^)]*\)/g, (m) =>              // links -> keep text
      m.replace(/\([^)]*\)/, '')
    )
    .replace(/(^|\s)[>*#_\-\+]{1,3}\s?/g, ' ')          // md markers
    .replace(/\s+/g, ' ')
    .trim();
}

async function buildDocs(): Promise<Doc[]> {
  const docs: Doc[] = [];
  let id = 1;

  for (const [filePath, raw] of Object.entries(rawModules)) {
    try {
      const rawText = raw as unknown as string;
      const { data, content } = parseFrontmatter(rawText);
      const title = (data['title'] as string) || toRoutePath(filePath);
      const description = (data['description'] as string) || '';
      const path = toRoutePath(filePath);
      const body = stripMarkdown(content);
      docs.push({ id: id++, path, title, description, body });
    } catch (err) {
      console.warn('[search] Failed to parse:', filePath, err);
    }
  }

  docs.sort((a, b) => a.path.localeCompare(b.path));
  return docs;
}

export async function ensureIndex(): Promise<void> {
  if (_mini && _docs) return;
  _docs = await buildDocs();

  _mini = new MiniSearch<Doc>({
    fields: ['title', 'body'],
    storeFields: ['title', 'path', 'description'],
    idField: 'id',
    searchOptions: {
      prefix: true,
      fuzzy: 0.1
    }
  });

  _mini.addAll(_docs);
  console.log('[search] Indexed docs:', _docs.length);
}

function makeSnippet(text: string, q: string): string {
  if (!text) return '';
  const query = q.trim().toLowerCase();
  const idx = text.toLowerCase().indexOf(query);
  if (idx === -1) return text.slice(0, 140) + (text.length > 140 ? '…' : '');
  const start = Math.max(0, idx - 50);
  const end = Math.min(text.length, idx + query.length + 50);
  return (start > 0 ? '…' : '') + text.slice(start, end) + (end < text.length ? '…' : '');
}

export type SearchResult = {
  title: string;
  path: string;
  description: string;
  score: number;
  snippet: string;
};

export async function doSearch(query: string): Promise<SearchResult[]> {
  await ensureIndex();
  if (!_mini || !_docs) return [];
  if (!query.trim()) return [];
  const res = _mini.search(query);
  return res.map((r) => {
    const doc = _docs!.find((d) => d.id === (r.id as number));
    return {
      title: (r as any).title || doc?.title || '',
      path: (r as any).path || doc?.path || '',
      description: (r as any).description || doc?.description || '',
      score: r.score,
      snippet: makeSnippet(doc?.body || '', query)
    };
  });
}

export function getIndexedCount(): number {
  return _docs?.length ?? 0;
}
