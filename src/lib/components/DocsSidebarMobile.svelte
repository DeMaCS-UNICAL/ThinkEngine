<script>
  import { Offcanvas, OffcanvasHeader, OffcanvasBody } from '@sveltestrap/sveltestrap';
  import { base } from '$app/paths';
  import { goto } from '$app/navigation';

  export let nav = [];
  export let open = false;
  export let onClose = () => {};

  const hrefFor = (p) => `${base}${p}`.replace(/\/{2,}/g, '/');

  const navigate = (p, e) => {
    e?.preventDefault();
    onClose();
    goto(hrefFor(p));
  };
</script>

<Offcanvas isOpen={open} toggle={onClose} placement="start">
  <OffcanvasHeader toggle={onClose}>Docs menu</OffcanvasHeader>
  <OffcanvasBody>
    <div class="list-group">
      {#each nav as item}
        <a
          href={hrefFor(item.path)}
          on:click|preventDefault={(e)=>navigate(item.path,e)}
          class="list-group-item list-group-item-action"
        >
          {item.title}
        </a>
      {/each}
    </div>
  </OffcanvasBody>
</Offcanvas>
