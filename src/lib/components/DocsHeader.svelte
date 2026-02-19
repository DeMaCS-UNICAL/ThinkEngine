<script>
  export let title = '';
  export let description = '';
</script>

<header class="mb-4">
  {#if title}<h1 class="h3 mb-2">{title}</h1>{/if}
  {#if description}<p class="text-muted mb-0">{description}</p>{/if}
</header>
