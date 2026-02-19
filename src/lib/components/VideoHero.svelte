<script>
  export let srcMp4 = '/videos/video-tutorial.mp4';
  export let poster = '/images/poster.png';
  export let title = 'ThinkEngine – Video Tutorial';

  let loaded = false;
  let videoRef;

  // Lazy load: mostra il player solo quando visibile
  function onVisible(node) {
    const observer = new IntersectionObserver((entries) => {
      if (entries.some(e => e.isIntersecting)) {
        loaded = true;
        observer.disconnect();
      }
    }, { rootMargin: '200px' });
    observer.observe(node);
    return { destroy() { observer.disconnect(); } };
  }

  const playVideo = () => {
    if (!loaded) loaded = true;
    // Avvia automaticamente dopo il click su play
    setTimeout(() => videoRef?.play(), 150);
  };
</script>

<!-- Contenitore responsive -->
<div use:onVisible class="ratio ratio-16x9 border rounded overflow-hidden shadow-sm">
  {#if loaded}
    <video
      class="w-100 h-100"
      controls
      preload="metadata"
      poster={poster}
      aria-label={title}
      bind:this={videoRef}
    >
      <source src={srcMp4} type="video/mp4" />
      Your browser does not support the video tag.
    </video>
  {:else}
    <!-- Placeholder con immagine e bottone play -->
    <div class="position-relative w-100 h-100">
      <img
        src={poster}
        alt={title}
        class="w-100 h-100"
        style="object-fit: cover;"
      />
      <button
        class="btn btn-light position-absolute top-50 start-50 translate-middle px-3 py-2 shadow-lg"
        on:click={playVideo}
        aria-label="Play video"
      >
        ▶ Play
      </button>
    </div>
  {/if}
</div>

<style>
  :global(video) {
    border-radius: 0.5rem;
    background-color: #000;
  }
  button:hover {
    background-color: #f8f9fa;
  }
</style>
