import type { LayoutLoad } from './$types';

type DocModule = {
  metadata?: Record<string, unknown>;
  default: unknown;
};

export const load: LayoutLoad = async () => {
  // importa tutti i .md sotto src/content
  const modules = import.meta.glob('/src/content/**/*.md');

  const entries = await Promise.all(
    Object.entries(modules).map(async ([path, resolver]) => {
      const mod = (await resolver()) as DocModule;

      // path pubblico /docs/.../
      let urlPath = path
        .replace('/src/content', '/docs')
        .replace(/\/index\.md$/, '/')
        .replace(/\.md$/, '/');

      urlPath = urlPath.replace(/\/{2,}/g, '/'); // no doppie slash
      if (!urlPath.endsWith('/')) urlPath += '/'; // trailing slash

      const meta = (mod as any).metadata || {};
      return {
        path: urlPath,
        title: (meta.title as string) || urlPath,
        order: (meta.order as number) ?? 999
      };
    })
  );

  // ordina: per path e poi per order
  const nav = entries
    .sort((a, b) => a.path.localeCompare(b.path))
    .sort((a, b) => a.order - b.order);

  return { nav };
};
