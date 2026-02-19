<script>
  import { doSearch, ensureIndex, getIndexedCount } from '$lib/search/search';
  import { goto } from '$app/navigation';
  import { base } from '$app/paths';
  import { onMount } from 'svelte';

  let q = '';
  let results = [];
  let open = false;
  let inputEl;
  let indexed = 0;
  let timer;

  onMount(async () => {
    await ensureIndex();
    indexed = getIndexedCount();
    console.log('[SearchBox] Ready, indexed:', indexed);
  });

  async function runSearch() {
    try {
      results = q.trim() ? (await doSearch(q)).slice(0, 10) : [];
      open = results.length > 0;
    } catch (e) {
      console.error('[SearchBox] search error', e);
      results = [];
      open = false;
    }
  }

  function onInput(e) {
    q = e.currentTarget.value;
    clearTimeout(timer);
    timer = setTimeout(runSearch, 150);
  }

  function go(path) {
    open = false;
    q = '';
    goto(`${base}${path}`);
  }
</script>

<div class="position-relative" style="min-width:260px; max-width:420px;">
  <input
    bind:this={inputEl}
    class="form-control"
    type="search"
    placeholder="Search docs"
    value={q}
    on:input={onInput}
    on:focus={() => (open = results.length > 0)}
  />

  

  {#if open}
    <div class="position-absolute bg-white border rounded shadow-sm w-100 mt-1" style="z-index: 1000;">
      {#if results.length === 0}
        <div class="p-2 text-muted small">No results</div>
      {:else}
        <ul class="list-unstyled mb-0">
          {#each results as r}
            <li>
              <a
                href={`${base}${r.path}`}
                on:click|preventDefault={() => go(r.path)}
                class="d-block px-3 py-2 text-decoration-none"
              >
                <div class="fw-semibold">{r.title}</div>
                {#if r.description}
                  <div class="text-muted small">{r.description}</div>
                {/if}
                {#if r.snippet}
                  <div class="small text-muted">{r.snippet}</div>
                {/if}
              </a>
            </li>
          {/each}
        </ul>
      {/if}
    </div>
  {/if}
</div>
