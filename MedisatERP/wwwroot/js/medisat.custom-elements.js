﻿// Wait for the DOM content to be loaded
document.addEventListener('DOMContentLoaded', function () {

    // This script provides custom templates of the application

    class WelcomeLogo extends HTMLElement {
        connectedCallback() {
            this.innerHTML = `
                <div class="text-center mt-4">
                    <h3>MEDISAT ERP</h3>
                 </div>
            `;
        }
    }
    customElements.define('welcome-logo', WelcomeLogo);

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

    class WelcomeFooter extends HTMLElement {
        connectedCallback() {
            this.innerHTML = `
            <!-- Footer with copyright information -->
            <div class="text-center mb-3">
                <p class="mt-3" style="font-size:11px;">&copy;Medisat ERP </p>
            </div>
            `;
        }
    }
    customElements.define('welcome-footer', WelcomeFooter);

    class EndBarSection extends HTMLElement {
        connectedCallback() {
            this.innerHTML = `
                <!-- Footer Section -->
                <footer class="footer">
                    <div class="container-fluid">
                        <div class="row text-muted">
                            <div class="col-12 text-start">
                                <p class="mb-0" style="color: #565e64; ">
                                    MEDISAT
                                </p>
                            </div>
                        </div>
                    </div>
                </footer>
            `;
        }
    }
    customElements.define('end-bar', EndBarSection);

});

