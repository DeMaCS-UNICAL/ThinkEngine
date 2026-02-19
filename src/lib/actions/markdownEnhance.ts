// src/lib/actions/markdownEnhance.ts
export function markdownEnhance(node: HTMLElement) {
  const enhance = () => {
    // Tabelle → Bootstrap + responsive wrapper
    const tables = Array.from(node.querySelectorAll('table'));
    for (const table of tables) {
      table.classList.add('table', 'table-striped', 'table-bordered', 'table-sm');
      const parent = table.parentElement;
      if (parent && !parent.classList.contains('table-responsive')) {
        const wrapper = document.createElement('div');
        wrapper.className = 'table-responsive';
        parent.replaceChild(wrapper, table);
        wrapper.appendChild(table);
      }
    }

    // Immagini → responsive
    node.querySelectorAll('img').forEach((img) => {
      img.classList.add('img-fluid');
    });

    // (Opzionale) bottone "Copy" sui blocchi di codice
    node.querySelectorAll('pre').forEach((pre) => {
      if (pre.querySelector('.copy-btn')) return;
      const btn = document.createElement('button');
      btn.textContent = '📋 Copy';
      btn.className = 'btn btn-sm btn-light position-absolute top-0 end-0 m-1 copy-btn';
      btn.onclick = () => {
        const code = pre.querySelector('code')?.textContent ?? '';
        navigator.clipboard.writeText(code);
        btn.textContent = '✅ Copied!';
        setTimeout(() => (btn.textContent = '📋 Copy'), 1600);
      };
      pre.style.position = 'relative';
      pre.appendChild(btn);
    });
  };

  enhance();
  return { update: enhance };
}
