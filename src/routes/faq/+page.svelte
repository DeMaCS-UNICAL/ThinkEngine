<script>
  import { Accordion, AccordionItem, Input } from '@sveltestrap/sveltestrap';
  import faqs from '$lib/data/faq.json'; // ✅ import diretto

  let search = '';
  let filteredFaqs = faqs;

  const filterFaqs = () => {
    const term = search.toLowerCase();
    filteredFaqs = faqs.filter(f =>
      f.q.toLowerCase().includes(term) || f.a.toLowerCase().includes(term)
    );
  };
</script>


<div class="container py-4">
  <h1 class="mb-4">❓ FAQ – Frequently Asked Questions</h1>

  <!-- Search box -->
  <div class="mb-3">
    <Input
      type="text"
      placeholder="Search FAQ..."
      bind:value={search}
      on:input={filterFaqs}
    />
  </div>

  {#if filteredFaqs.length > 0}
    {#each [...new Set(filteredFaqs.map(f => f.category))] as cat}
      <h4 class="mt-4 mb-2">{cat}</h4>
      <Accordion stayOpen>
        {#each filteredFaqs.filter(f => f.category === cat) as item}
          <AccordionItem>
            <div slot="header">{item.q}</div>
            <p class="mb-0">{item.a}</p>
          </AccordionItem>
        {/each}
      </Accordion>
    {/each}
  {:else}
    <p>No results found.</p>
  {/if}
</div>
