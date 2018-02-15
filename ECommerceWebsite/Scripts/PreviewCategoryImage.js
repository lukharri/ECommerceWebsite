$(function () {

    // preview product image
    function readUrl(input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $("img#image-preview")
                    .attr("src", e.target.result)
                    .width(200)
                    .height(200);
            };

            reader.readAsDataURL(input.files[0]);
        }
    }

    $("#image-upload").change(function () {
        readUrl(this);
    });
});
