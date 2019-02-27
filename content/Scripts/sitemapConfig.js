$(document).ready(function () {
    loadDataEngines();
});

//Load Data function
function loadDataEngines() {
    $.ajax({
        url: "/SitemapPlugin/ListSearchEngines",
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            var html = '';
            $.each(result, function (key, item) {
                html += '<tr>';
                html += '<td>' + item.Name + '</td>';
                html += '<td>' + item.Url + '</td>';
                html += '<td><a href="#" onclick="return getEnginebyID(\'' + item.Code + '\')">Edit</a> | <a href="#" onclick="DeleteEngine(\'' + item.Code + '\')">Delete</a></td>';
                html += '</tr>';
            });

            $('#enginesBody').html(html);
        },

        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}

//Add Data Function
function AddEngine() {
    var res = validateEngine();
    if (res === false) {
        return false;
    }

    var engObj = {
        Id: $('#Code').val(),
        Name: $('#Name').val(),
        Url: $('#Url').val()
    };

    $.ajax({
        url: "/SitemapPlugin/AddSearchEngine",
        data: JSON.stringify(engObj),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",

        success: function (result) {
            loadDataEngines();
            $('#engineModal').modal('hide');
        },

        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}

//Function for getting the Data Based upon Employee ID
function getEnginebyID(code) {
    $('#Id').css('border-color', 'lightgrey');
    $('#Name').css('border-color', 'lightgrey');
    $('#Url').css('border-color', 'lightgrey');

    $.ajax({
        url: "/SitemapPlugin/GetSearchEngineById/" + code,
        type: "GET",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",

        success: function (result) {
            $('#Code').val(result.Code);
            $('#Name').val(result.Name);
            $('#Url').val(result.Url);
            $('#engineModal').modal('show');
            $('#btnUpdate').show();
            $('#btnAdd').hide();
        },

        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });

    return false;
}

//function for updating employee's record
function UpdateEngine() {
    var res = validateEngine();
    if (res === false) {
        return false;
    }

    var engObj = {
        Code: $('#Code').val(),
        Name: $('#Name').val(),
        Url: $('#Url').val()
    };

    $.ajax({
        url: "/SitemapPlugin/UpdateSearchEngine",
        data: JSON.stringify(engObj),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            loadDataEngines();
            $('#engineModal').modal('hide');
            $('#Id').val("");
            $('#Name').val("");
            $('#Url').val("");
        },

        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}

//function for deleting employee's record
function DeleteEngine(code) {
    var ans = confirm("Are you sure you want to delete this Record?");
    if (ans) {
        $.ajax({
            url: "/SitemapPlugin/DeleteSearchEngine/" + code,
            type: "POST",
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function (result) {
                loadDataEngines();
            },
            error: function (errormessage) {
                alert(errormessage.responseText);
            }
        });
    }
}

//Function for clearing the textboxes

function clearTextBoxEngine() {
    $('#Code').val("");
    $('#Name').val("");
    $('#Url').val("");
    $('#btnUpdate').hide();
    $('#btnAdd').show();
    $('#Name').css('border-color', 'lightgrey');
    $('#Url').css('border-color', 'lightgrey');
}

//Valdidation using jquery
function validateEngine() {
    var pattern = /^(http(s)?:\/\/)?(www\.)?[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$/;

    var isValid = true;
    if ($('#Name').val().trim() === "") {
        $('#Name').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#Name').css('border-color', 'lightgrey');
    }

    var url = $('#Url').val().trim();
    if (url === "" || !pattern.test(url)) {
        $('#Url').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#Url').css('border-color', 'lightgrey');
    }

    return isValid;
}