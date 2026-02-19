<script>
  import { onMount } from 'svelte';
  let visible = false;

  const onScroll = () => {
    visible = window.scrollY > 300;
  };

  const toTop = () => {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  onMount(() => {
    onScroll();
    window.addEventListener('scroll', onScroll, { passive: true });
    return () => window.removeEventListener('scroll', onScroll);
  });
</script>

<button
  class="btn btn-primary position-fixed"
  style="right:1rem; bottom:1rem; z-index:1040; opacity:{visible ? 1 : 0}; pointer-events:{visible ? 'auto' : 'none'}; transition:opacity .2s;"
  on:click={toTop}
  aria-label="Back to top"
>
  ⬆ Back to top
</button>

<style>
  :global(html) { scroll-behavior: smooth; } /* fallback se qualcuno chiama #ancore */
</style>
