import adapter from '@sveltejs/adapter-static';
import { mdsvex } from 'mdsvex';
import gfm from 'remark-gfm';
import rehypeHighlight from 'rehype-highlight';
import rehypeSlug from 'rehype-slug';

const config = {
  extensions: ['.svelte', '.svx', '.md'],
  preprocess: [
    mdsvex({
      extensions: ['.svx', '.md'],
      smartypants: false,
      remarkPlugins: [gfm],
      rehypePlugins: [
        [rehypeHighlight, { ignoreMissing: true, detect: true }],
        rehypeSlug
      ]
    })
  ],
  kit: {
    adapter: adapter({
      pages: 'build',
      assets: 'build',
      fallback: '404.html'
    }),
    alias: {
      $content: 'src/content',
      $lib: 'src/lib'
    },
    paths: {
      base: '/ThinkEngine'
    },
    prerender: {
      entries: ['*']
    }
  }
};

export default config;