import type { PageLoad } from './$types';

type DocModule = {
  metadata?: Record<string, unknown>;
  default: unknown;
};

const modules = import.meta.glob('/src/content/**/*.md');

export const load: PageLoad = async ({ params }) => {
  const parts = params.slug ? params.slug.split('/').filter(Boolean) : [];

  const candidates = [
    `/src/content/${parts.join('/')}/index.md`,
    `/src/content/${parts.join('/')}.md`
  ];

  for (const path of candidates) {
    const resolver = modules[path];
    if (resolver) {
      const mod = (await resolver()) as DocModule;
      return {
        component: mod.default,
        metadata: (mod as any).metadata || {},
        importId: path
      };
    }
  }

  return {
    component: null,
    metadata: { title: 'Not found' },
    importId: '404'
  };
};