var dataTable;

$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax" : {
            "url" : "/Admin/Product/GetAll"
        },
        "columns": [
            { "data": "title", "width": "15%" },
            { "data": "isbn", "width": "15%" },
            { "data": "price", "width": "15%" },
            { "data": "author", "width": "15%" },
            { "data": "category.name", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <td>
                            <a href="/Admin/Product/Upsert?id=${data}" class="btn btn-primary"><i class="bi bi-pencil-square"></i></a>
                            <a onClick=Delete('/Admin/Product/Delete/${data}') class="btn btn-danger"><i class="bi bi-trash3"></i></a>
                        </td>
                        `
                },
                "width": "15%",
            },
        ]
    });
}

function Delete (url){
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                succsess: function (data) {
                    if (data.succsess) {
                        // TODO this does not work god knows why, maybe I will fix this
                        $('#tblData').DataTable().ajax.reload()
                        toastr.succsess(data.message);
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}