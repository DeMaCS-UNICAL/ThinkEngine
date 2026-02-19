<script>
  import { Navbar, NavbarBrand, Nav, NavItem, NavLink } from '@sveltestrap/sveltestrap';
  import { base } from '$app/paths';
  import SearchBox from '$lib/components/SearchBox.svelte';
</script>

<Navbar color="light" light expand="md" class="border-bottom">
  <div class="container-fluid">
    <!-- Brand -->
    <NavbarBrand href={`${base}/`}>ThinkEngine Docs</NavbarBrand>

    <!-- Link principali -->
    <Nav class="ms-auto me-3" navbar>
      <NavItem>
        <NavLink href={`${base}/docs/01-overview/`}>Docs</NavLink>
      </NavItem>
      <NavItem>
    <NavLink href={`${base}/faq`}>FAQ</NavLink>
  </NavItem>
      <NavItem>
        <NavLink href="https://github.com/tuo-repo" target="_blank" rel="noopener noreferrer">
          GitHub
        </NavLink>
      </NavItem>
      <NavItem>
        <NavLink href="https://thinkengine.ai">Website</NavLink>
      </NavItem>
    </Nav>

    <!-- Search -->
    <SearchBox />
  </div>
</Navbar>
