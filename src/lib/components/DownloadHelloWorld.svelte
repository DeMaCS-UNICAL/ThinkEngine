<script>
  import { onMount } from 'svelte';
  import { base } from '$app/paths';

  export let file = 'ThinkEngineMicro.zip'; // nome dello zip in /static/downloads
  export let icon = '⬇';
  export let buttonText = 'ThinkEngine Hello World';

  let meta = null;
  let error = '';
  let showHash = false;

  function humanSize(bytes) {
    if (!bytes && bytes !== 0) return '';
    const units = ['B', 'KB', 'MB', 'GB', 'TB'];
    let i = 0;
    let n = bytes;
    while (n >= 1024 && i < units.length - 1) {
      n /= 1024;
      i++;
    }
    return `${n.toFixed(n < 10 && i > 0 ? 1 : 0)} ${units[i]}`;
  }

  onMount(async () => {
    try {
      const res = await fetch(`${base}/downloads/checksums.json`, { cache: 'no-store' });
      if (!res.ok) throw new Error('checksums.json not found');
      const json = await res.json();
      meta = (json.artifacts || []).find(a => a.file === file) || null;
      if (!meta) throw new Error(`Artifact "${file}" not found in checksums.json`);
    } catch (e) {
      error = e.message || String(e);
    }
  });

  const href = `${base}/downloads/${file}`;
  const aria = `Download ${meta?.displayName || buttonText} (ZIP)`;
</script>

<div class="mt-3">
  {#if error}
    <div class="alert alert-warning small mb-2">⚠ {error}</div>
  {/if}

  <a
    class="btn btn-success"
    href={href}
    download
    aria-label={aria}
  >
    {icon} {meta?.displayName ?? buttonText}
  </a>

  {#if meta}
    <div class="text-muted small mt-2">
      {#if meta.version}<span class="me-2">v{meta.version}</span>{/if}
      {#if meta.size}<span class="me-2">({humanSize(meta.size)})</span>{/if}
      {#if meta.released}<span class="me-2">Released: {meta.released}</span>{/if}
      <button class="btn btn-link btn-sm p-0 align-baseline" on:click={() => (showHash = !showHash)}>
        {showHash ? 'Hide SHA256' : 'Show SHA256'}
      </button>
      {#if showHash && meta.sha256}
        <div class="mt-1"><code class="user-select-all">{meta.sha256}</code></div>
      {/if}
    </div>
  {/if}
</div>
