<script>
  import { base } from '$app/paths';
    import DownloadHelloWorld from '$lib/components/DownloadHelloWorld.svelte';
  import VideoHero from '$lib/components/VideoHero.svelte';
</script>

<div class="container py-5">
  <div class="row g-4 align-items-center">
    <div class="col-12 col-lg-6">
      <h1 class="mb-3">ThinkEngine Docs</h1>
      <p class="text-muted mb-4">
        Watch the short tutorial below or go straight to the documentation.
      </p>
      <a class="btn btn-primary me-2 mb-2" href={`${base}/docs/01-overview/`}>
        Open Conceptual Overview →
      </a>
      <a class="btn btn-outline-secondary mb-2" href={`${base}/faq`}>FAQ</a>
      <DownloadHelloWorld file="ThinkEngineMicro.zip" />
    </div>
    <div class="col-12 col-lg-6">
      <VideoHero
        srcMp4="videos/video-tutorial.mp4"
        poster="images/poster.png"
        title="ThinkEngine – Video Tutorial"
      />
    </div>
  </div>
</div>
