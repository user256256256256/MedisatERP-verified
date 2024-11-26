// Wait for the DOM content to be loaded
document.addEventListener('DOMContentLoaded', function () {

    // This script provides custom templates of the system

    class SideBarBrand extends HTMLElement {
        connectedCallback() {
            this.innerHTML = `
                <!-- Sidebar Brand (Logo) -->
                <a class="sidebar-brand" href="/">
                    <center><span class="align-middle">MEDISAT ERP </span></center>
                </a>
            `;
        }
    }
    customElements.define('sidebar-brand', SideBarBrand);

});
