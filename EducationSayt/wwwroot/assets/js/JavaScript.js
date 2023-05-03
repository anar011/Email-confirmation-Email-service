$(function () {

    $(document).on("click", ".trash-icon i", function (e) {

        let imageId = $(this).parent().attr("data-id");
        let deletedElem = $(this).parent();
        let data = { id: imageId };

        $.ajax({
            url: "/Admin/Course/DeleteProductImage",
            type: "Post",
            data: data,
            success: function (res) {
                if (res) {
                    $(deletedElem).remove();
                } else {
                    alert("Product images must be min 1")
                }

            }

        })
    })
})