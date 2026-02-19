<script>
  import DocsSidebar from '$lib/components/DocsSidebar.svelte';
  import DocsBreadcrumbs from '$lib/components/DocsBreadcrumbs.svelte';
  import DocsSidebarMobile from '$lib/components/DocsSidebarMobile.svelte';
  import BackToTop from '$lib/components/BackToTop.svelte';
  import { setContext } from 'svelte';

  export let data; // { nav }
  const { nav } = data;

  // mettiamo la nav in context per i componenti figli (es. breadcrumbs)
  setContext('docs:nav', nav);

  let mobileOpen = false;
  const openMobile = () => (mobileOpen = true);
  const closeMobile = () => (mobileOpen = false);
</script>

<div class="container-fluid">
  <div class="row">
    <!-- Sidebar desktop -->
    <DocsSidebar {nav} />

    <main class="col-12 col-md-9 col-lg-10 p-3 p-md-4">
      <!-- Header strip per mobile -->
      <div class="d-md-none d-flex align-items-center justify-content-between mb-2">
        <button class="btn btn-outline-primary btn-sm" on:click={openMobile}>☰ Menu</button>
      </div>

      <!-- Breadcrumbs sempre visibili sopra al contenuto -->
      <DocsBreadcrumbs />

      <!-- Contenuto delle pagine -->
      <slot />

      <!-- Back to top -->
      <BackToTop />
    </main>
  </div>
</div>

<!-- Offcanvas mobile -->
<DocsSidebarMobile {nav} open={mobileOpen} onClose={closeMobile} />
