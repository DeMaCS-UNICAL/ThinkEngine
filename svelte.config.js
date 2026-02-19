import adapter from '@sveltejs/adapter-static';
import { mdsvex } from 'mdsvex';
import gfm from 'remark-gfm';
import rehypeHighlight from 'rehype-highlight';
import rehypeSlug from 'rehype-slug'; // 👈 NEW

const config = {
  extensions: ['.svelte', '.svx', '.md'],
  preprocess: [
    mdsvex({
      extensions: ['.svx', '.md'],
      smartypants: false,
      remarkPlugins: [gfm],
      rehypePlugins: [
        [rehypeHighlight, { ignoreMissing: true, detect: true }],
        rehypeSlug // 👈 aggiunge id agli <h1..h6>
      ]
    })
  ],
  kit: {
    adapter: adapter({ pages: 'build', assets: 'build', fallback: '404.html' }),
    alias: { $content: 'src/content', $lib: 'src/lib' },
    paths: { base: process.env.PUBLIC_BASE_PATH || '' },
    prerender: { entries: ['*'] }
  }
};

export default config;
