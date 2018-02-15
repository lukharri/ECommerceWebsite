$(function () {
    $("#select-category").on("click", function () {
        var url = $(this).val();

        if (url)
            window.location = "/admin/shop/products?catId=" + url;

        return false;
    });
});