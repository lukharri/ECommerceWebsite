$(function () {

    // Add new category
    var newCatA = $("a#new-cat-a");
    var newCatTextInput = $("#new-cat-name");
    var ajaxText = $("span.ajax-text");
    var table = $("table#pages tbody");

    newCatTextInput.keyup(function (e) {
        if (e.keyCode == 13) {
            newCatA.click();
        }
    });

    newCatA.click(function (e) {
        e.preventDefault();

        var catName = newCatTextInput.val();

        if (catName.length < 2) {
            alert("Category name must be at least 2 characters long.");
            return false;
        }

        ajaxText.show();

        var url = "/admin/shop/AddNewCategory";

        // AJAX call for creating a new category
        $.post(url, { catName: catName }, function (data) {
            var response = data.trim();

            // check if category name already exists
            if (response == 'titletaken') {
                ajaxText.html("<span class='alert alert-danger'>Category name already exists.</span>");
                setTimeout(function () {
                    ajaxText.fadeOut("fast", function () {
                        ajaxText.html("<img src=/Content/img/ajax-loader.gif />");
                    });
                }, 2000);
                return false;

                // check if categories table is present before adding another row to it
            } else {
                if (!$("table#pages").length) {
                    location.reload();
                } else {
                    ajaxText.html("<span class='alert alert-success'>Category successfully created.");
                    setTimeout(function () {
                        ajaxText.fadeOut("fast", function () {
                            ajaxText.html("<img src=/Content/img/ajax-loader.gif />");
                        });
                    }, 2000);

                    newCatTextInput.val("");

                    // Add a new row to category table w/category name
                    var toAppend = $("table#pages tbody tr:last").clone();
                    toAppend.attr("id", "id_" + data);
                    toAppend.find("#item_Name").val(catName);
                    toAppend.find("a.delete").attr("href", "/Admin/Shop/DeleteCategory/" + data);
                    table.append(toAppend);
                    table.sortable("refresh");
                }
            }
        });
    });

    //*****************************************************************************************************//


    // Delete categories
    $("body").on("click", "a.delete", function () {
        if (!confirm("Confirm category deletion")) return false;
    });


    //*****************************************************************************************************//


    // Rename categories
    var originalTextBoxValue;
    $("table#pages input.text-box").dblclick(function () {
        originalTextBoxValue = $(this).val();
        $(this).attr("readonly", false);
    });

    // trigger blur effect on enter
    $("table#pages input.text-box").keyup(function (e) {
        if (e.keyCode == 13) {
            $(this).blur();
        }
    });

    $("table#pages input.text-box").blur(function () {
        var $this = $(this);
        var ajaxDiv = $this.parent().find(".ajax-div-td");
        var newCatName = $this.val();
        var id = $this.parent().parent().attr("id").substring(3);
        var url = "/admin/shop/RenameCategory";

        if (newCatName.length < 2) {
            alert("Category name must be at least 2 characters long.");
            $this.attr("readonly", true);
            return false;
        }

        $.post(url, { newCatName: newCatName, id: id }, function (data) {
            var resonse = data.trim();

            if (response == "titletaken") {
                $this.val(originalTextBoxValue);
                ajaxDiv.html("<span class='alert alert-danger'>Category name already exists.</span>").show();
            } else {
                ajaxDiv.html("<span class='alert alert-success'>Category named successfully updated.</span>").show();
            }

            setTimeout(function () {
                ajaxDiv.fadeOut("fast", function () { 
                    ajaxDiv.html("");
                })
            }, 2000);

            }).done(function () {
                $this.attr("readonly", true);
        });

    });

    //*****************************************************************************************************//


    // DRAG N' DROP function for sorting categories  
    // http://api.jqueryui.com/sortable/
    $("table#pages tbody").sortable({
        items: "tr:not(.home)",
        placeholder: "ui-state-highlight",
        update: function () {
            var ids = $("table#pages tbody").sortable("serialize");
            var url = "/Admin/Shop/ReorderCategories";

            $.post(url, ids, function () {

            });
        }
    });

    //*****************************************************************************************************//

});
