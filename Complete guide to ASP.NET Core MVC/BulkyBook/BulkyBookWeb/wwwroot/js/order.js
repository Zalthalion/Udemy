var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    // bruh these ifs suck
    if (url.includes("inprocess")) {
        loadDataTable("inprocess");
    }
    else {
        if (url.includes("completed")) {
            loadDataTable("completed");

        }
        else {
            if (url.includes("pending")) {
                loadDataTable("pending");
            }
            else {
                if (url.includes("approved")) {
                    loadDataTable("approved");
                }
                else {
                    loadDataTable("all");
                }
            }
        }
    }    
})

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "ajax" : {
            "url" : "/Admin/Order/GetAll?status=" + status
        },
        "columns": [
            { "data": "id", "width": "5%" },
            { "data": "aplicationUser.name", "width": "25%" },
            { "data": "aplicationUser.phoneNumber", "width": "15%" },
            { "data": "aplicationUser.email", "width": "15%" },
            { "data": "orderStatuss", "width": "15%" },
            { "data": "orderTotal", "width": "10%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <td>
                            <a href="/Admin/Order/Details?id=${data}" class="btn btn-primary"><i class="bi bi-pencil-square"></i></a>
                        </td>
                        `
                },
                "width": "5%",
            },
        ]
    });
}