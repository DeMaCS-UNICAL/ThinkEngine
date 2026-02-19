import type { PageLoad } from './$types';

type DocModule = {
  metadata?: Record<string, unknown>;
  default: unknown;
};

export const load: PageLoad = async ({ params }) => {
  const parts = params.slug ? params.slug.split('/').filter(Boolean) : [];
  const base = '/src/content';

  const candidates = [
    `${base}/${parts.join('/')}/index.md`,
    `${base}/${parts.join('/')}.md`
  ];

  for (const path of candidates) {
    try {
      const mod = (await import(/* @vite-ignore */ path)) as DocModule;
      return {
        component: mod.default,
        metadata: (mod as any).metadata || {},
        importId: path // chiave unica per forzare il remount
      };
    } catch {
      // prova il prossimo
    }
  }

  return {
    component: null,
    metadata: { title: 'Not found' },
    importId: '404'
  };
};
