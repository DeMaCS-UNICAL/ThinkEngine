<script>
  import DocsHeader from '$lib/components/DocsHeader.svelte';
  import DocsTOC from '$lib/components/DocsTOC.svelte';
  import { markdownEnhance } from '$lib/actions/markdownEnhance';

  export let data;

  // REATTIVO: aggiorna quando cambia pagina
  $: Doc = data?.component || null;
  $: title = data?.metadata?.title ?? '';
  $: description = data?.metadata?.description ?? '';

  let articleEl;
</script>

{#if Doc}
  <DocsHeader {title} {description} />

  <div class="row">
    <div class="col-12 col-lg-9">
      
      <article bind:this={articleEl} use:markdownEnhance>
        {#key data.importId}
          <svelte:component this={Doc} />
        {/key}
      </article>
    </div>
    <div class="d-none d-lg-block col-lg-3">
      <DocsTOC container={articleEl} />
    </div>
  </div>
{:else}
  <h1 class="h4">404 — Not found</h1>
  <p>The requested document does not exist.</p>
{/if}
