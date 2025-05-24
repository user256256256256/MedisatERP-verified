// Wait for the DOM content to be loaded
document.addEventListener('DOMContentLoaded', function () {

    // This script provides custom templates of the application

    class WelcomeLogo extends HTMLElement {
        connectedCallback() {
            this.innerHTML = `
                <div class="text-center mt-4">
                    <img class="welcome-logo" src="/img/companyLogoImages/logo-2-tp.png" alt="medisat-logo-1" style="max-height:8rem; max-width:8rem;" />
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
                    <center>
                        <img class="welcome-logo" src="/img/companyLogoImages/logo-1.jpg" alt="medisat-logo-1" style="max-height:10rem; max-width:10rem;" />
                    </center>
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

    class SecurityHelpText extends HTMLElement {
        connectedCallback() {
            this.innerHTML = `
                <div class="text-center text-secondary mt-3 mb-3 px-2" style="max-width: 300px; margin: 0 auto; word-wrap: break-word; overflow-wrap: break-word;">
                    <small>If you're having trouble, contact <strong>security@medisat.com</strong> for further assistance.</small>
                </div>
            `;
        }
    }
    customElements.define('help-text', SecurityHelpText);


});

