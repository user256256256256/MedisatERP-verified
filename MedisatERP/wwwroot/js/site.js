(function (window, document, $) {
    $(function () {
        var sidebarNav = $(".sidebar-nav");
        if (sidebarNav.length > 0) {
            $('#sidebarNav').metisMenu();  // Assuming you're using the MetisMenu jQuery plugin
        }

        // Sidebar mobile toggle (if applicable)
        $('.js-sidebar-toggle').on('click', function () {
            // Toggle the 'collapsed' class on the sidebar
            $('.sidebar').toggleClass('collapsed');
        });

        // Prevent click propagation in mega-menu dropdown
        $(document).on('click', '.mega-menu.dropdown-menu', function (e) {
            e.stopPropagation();
        });

        // Toggle mini sidebar and navbar expansion
        $('.sidebar-toggle').on('click', function () {
            $('body').toggleClass('sidebar-mini');
            $('.app-navbar').toggleClass('expand');
        });

        // Hover effect on navbar (only when sidebar is mini)
        $('.app-navbar').hover(function () {
            if ($('body').hasClass('sidebar-mini')) {
                $('.navbar-header').toggleClass('expand');
            }
        });

        // Show search wrapper when search icon is clicked
        $('.search').on('click', function () {
            $('.search-wrapper').fadeIn(200);
        });

        // Hide search wrapper when close button is clicked
        $('.close-btn').on('click', function () {
            $('.search-wrapper').fadeOut(200);
        });
       
        // Check if any sidebar item has the 'active' class
        $(".sidebar-item").each(function () {
            if ($(this).hasClass("active")) {
                // Get the parent <ul> of the active <li> and add 'show' class
                $(this).closest("ul").addClass("show");
            }
        });

        // Add active class on click to the sidebar header
        $(".sidebar-header").on("click", function () {
            // Remove 'active' class from all sidebar headers
            $(".sidebar-header").removeClass("active");

            // Add 'active' class to the clicked sidebar header
            $(this).addClass("active");
        });

    });

})(window, document, window.jQuery);
