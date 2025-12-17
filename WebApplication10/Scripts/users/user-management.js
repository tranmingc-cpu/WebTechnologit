$(document).ready(function () {

    // Function to load user list via AJAX
    function loadUserList() {
        $.ajax({
            url: '/Users/UserListPartial',
            type: 'GET',
            success: function (data) {
                $('#user-list').html(data);
            },
            error: function () {
                alert('Failed to load user list.');
            }
        });
    }

    // Initial load
    loadUserList();

    // Open Create User Modal
    $('#btn-create-user').click(function () {
        $.ajax({
            url: '/Users/Create',
            type: 'GET',
            success: function (data) {
                $('#modal-content').html(data);
                $('#userModal').modal('show');
            },
            error: function () {
                alert('Failed to load create user form.');
            }
        });
    });

    // Submit Create/Edit form via AJAX (delegated because form is dynamically loaded)
    $(document).on('submit', '#createUserForm, #editUserForm', function (e) {
        e.preventDefault();
        var $form = $(this);

        $.ajax({
            url: $form.attr('action'),
            type: 'POST',
            data: $form.serialize(),
            success: function (result) {
                if (result.success) {
                    $('#userModal').modal('hide');
                    loadUserList();
                } else if (result.errors) {
                    var $form = $('#createUserForm, #editUserForm');
                    var errorHtml = '<div class="alert alert-danger"><ul>';
                    $.each(result.errors, function (i, error) {
                        errorHtml += '<li>' + error + '</li>';
                    });
                    errorHtml += '</ul></div>';

                    // Remove any previous error messages
                    $form.find('.alert-danger').remove();

                    $form.prepend(errorHtml);
                } else {
                    alert('Save failed.');
                }
            },
            error: function () {
                alert('Failed to save user.');
            }
        });
    });

    // Open Edit User Modal
    $(document).on('click', '.btn-edit', function () {
        var userId = $(this).data('id');
        $.ajax({
            url: '/Users/Edit/' + userId,
            type: 'GET',
            success: function (data) {
                $('#modal-content').html(data);
                $('#userModal').modal('show');
            },
            error: function () {
                alert('Failed to load edit form.');
            }
        });
    });

    // Open Delete User Modal
    $(document).on('click', '.btn-delete', function () {
        var userId = $(this).data('id');
        $.ajax({
            url: '/Users/Delete/' + userId,
            type: 'GET',
            success: function (data) {
                $('#modal-content').html(data);
                $('#userModal').modal('show');
            },
            error: function () {
                alert('Failed to load delete confirmation.');
            }
        });
    });

    // Confirm Delete User
    $(document).on('click', '#confirm-delete', function () {
        var userId = $(this).data('id');

        $.ajax({
            url: '/Users/Delete/' + userId,
            type: 'POST',
            data: {
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (response) {
                if (response.success) {
                    $('#userModal').modal('hide');
                    loadUserList();
                } else {
                    alert(response.message || "Delete failed.");
                }
            },
            error: function (xhr) {
                alert('Delete failed: ' + xhr.statusText);
            }

        });
    });

});
