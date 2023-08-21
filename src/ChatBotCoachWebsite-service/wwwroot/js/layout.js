$(document).ready(function () {
    // Login Modal AJAX Handling
    $('#loginModal').on('show.bs.modal', function (e) {
        const modal = $(this);
        const targetURL = $(e.relatedTarget).data('url');
        const modalTitle = $(e.relatedTarget).data('title');

        modal.find('.modal-title').text(modalTitle);

        if (modal.find('.modal-body').is(':empty') || $(e.relatedTarget).data('force-reload') === true) {
            $.get(targetURL, function (data) {
                modal.find('.modal-body').html(data);
            }).fail(function (jqXHR, textStatus, errorThrown) {
                console.error("Error loading login form:", textStatus, errorThrown);
            });
        }
    });

    // Register Modal AJAX Handling
    $('#registerModal').on('show.bs.modal', function (e) {
        const modal = $(this);
        const targetURL = $(e.relatedTarget).data('url');
        const modalTitle = $(e.relatedTarget).data('title');

        modal.find('.modal-title').text(modalTitle);

        if (modal.find('#ajax-content-register').is(':empty') || $(e.relatedTarget).data('force-reload') === true) {
            $.get(targetURL, function (data) {
                modal.find('#ajax-content-register').html(data); // Adjust this line
            }).fail(function (jqXHR, textStatus, errorThrown) {
                console.error("Error loading register form:", textStatus, errorThrown);
            });
        }
    });

});