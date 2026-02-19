<script>
  import { onMount } from 'svelte';
  export let container; // viene passato da +page.svelte (bind:this={articleEl})

  let items = [];
  let activeId = '';

  function collect() {
    if (!container) return (items = []);
    const hs = container.querySelectorAll('h1[id], h2[id], h3[id]');
    items = Array.from(hs).map((el) => ({
      id: el.id,
      text: el.textContent?.trim() ?? '',
      level: Number(el.tagName.substring(1)) // 1,2,3
    }));
  }

  // ScrollSpy semplice
  let io;
  function observe() {
    if (!container || items.length === 0) return;
    io?.disconnect();
    io = new IntersectionObserver(
      (entries) => {
        const visible = entries
          .filter((e) => e.isIntersecting)
          .sort((a, b) => b.target.getBoundingClientRect().top - a.target.getBoundingClientRect().top);
        if (visible[0]) activeId = visible[0].target.id;
      },
      { rootMargin: '0px 0px -70% 0px', threshold: [0, 1] }
    );
    items.forEach((i) => {
      const el = container.querySelector('#' + CSS.escape(i.id));
      if (el) io.observe(el);
    });
  }

  onMount(() => {
    collect();
    observe();
    const mo = new MutationObserver(() => {
      collect();
      observe();
    });
    mo.observe(container, { childList: true, subtree: true });
    return () => {
      io?.disconnect();
      mo.disconnect();
    };
  });

  function indent(level) {
    if (level >= 3) return 'ps-4';
    if (level === 2) return 'ps-3';
    return '';
  }
</script>

<div class="position-sticky" style="top: 1rem;">
  <div class="fw-semibold mb-2">On this page</div>
  <ul class="list-unstyled small">
    {#each items as it}
      <li class={indent(it.level)}>
        <a
          href={'#' + it.id}
          class="text-decoration-none"
          aria-current={activeId === it.id ? 'true' : 'false'}
          style="display:block; padding:.2rem 0; {activeId === it.id ? 'font-weight:600;' : ''}"
        >
          {it.text}
        </a>
      </li>
    {/each}
  </ul>
</div>
