<script>
  import { page } from '$app/stores';
  import { base } from '$app/paths';
  import { goto } from '$app/navigation';

  export let nav = []; // [{ path: "/docs/01-overview/", title: "..." }]

  // URL corrente aggiornato dal router
  $: current = $page.url.pathname;

  // normalizza per confronto robusto (toglie lo slash finale tranne che per "/")
  const norm = (p) => {
    if (!p) return '/';
    const noDouble = p.replace(/\/{2,}/g, '/');
    if (noDouble.length > 1 && noDouble.endsWith('/')) return noDouble.slice(0, -1);
    return noDouble;
  };

  // costruisce href completo con base e trailing slash coerente
  const hrefFor = (p) => {
    let h = `${base}${p}`.replace(/\/{2,}/g, '/');
    if (!h.endsWith('/')) h += '/';
    return h;
  };

  // navigazione SPA esplicita
  const navigate = (p, e) => {
    e?.preventDefault();
    goto(hrefFor(p));
  };
</script>

<aside class="d-none d-md-block col-md-3 col-lg-2 border-end vh-100 overflow-auto pt-3">
  <ul class="list-group list-group-flush">
    {#each nav as item (item.path)}
      <a
        href={hrefFor(item.path)}
        on:click|preventDefault={(e) => navigate(item.path, e)}
        class="list-group-item list-group-item-action text-decoration-none"
        class:active={norm(current) === norm(hrefFor(item.path))}
      >
        {item.title}
      </a>
    {/each}
  </ul>
</aside>

<style>
  :global(.list-group-item.active) {
    background-color: #0d6efd;
    border-color: #0d6efd;
    color: #fff;
  }
</style>
