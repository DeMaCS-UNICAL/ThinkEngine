<script>
  import { page } from '$app/stores';
  import { base } from '$app/paths';
  import { getContext } from 'svelte';

  // nav proviene dal layout docs tramite setContext
  const nav = getContext('docs:nav') || [];

  // pathname corrente (include base)
  $: pathname = $page.url.pathname;

  // toglie il base dal pathname per confrontare con nav.path (che NON include base)
  const stripBase = (p) => (base && p.startsWith(base) ? p.slice(base.length) || '/' : p);

  // costruisce i crumbs /docs/.../
  $: crumbs = (() => {
    const p = stripBase(pathname);
    if (!p.startsWith('/docs')) return [];
    // assicura trailing slash
    const clean = p.endsWith('/') ? p : p + '/';
    const parts = clean.replace(/^\/docs\/?/, '').split('/').filter(Boolean);

    const items = [{ path: '/docs/', title: 'Docs' }];
    let acc = '/docs/';
    for (const seg of parts) {
      acc += seg + '/';
      const match = nav.find((n) => n.path === acc);
      items.push({
        path: acc,
        title: match?.title || seg
      });
    }
    return items;
  })();

  const hrefFor = (p) => `${base}${p}`.replace(/\/{2,}/g, '/');
</script>

<nav aria-label="breadcrumb" class="mb-3">
  <ol class="breadcrumb mb-0">
    {#each crumbs as c, i}
      {#if i < crumbs.length - 1}
        <li class="breadcrumb-item">
          <a class="text-decoration-none" href={hrefFor(c.path)}>{c.title}</a>
        </li>
      {:else}
        <li class="breadcrumb-item active" aria-current="page">{c.title}</li>
      {/if}
    {/each}
  </ol>
</nav>
