function loadListPage(pageNumber, displayNumber) {
    $("#listSection").empty();
    $("#listSection").append("Loading...");
    $("#listSection").load("List/", { pageNumber: pageNumber, displayNumber: displayNumber });
}

$(document).on("change",
    "#displayNumberSelection",
    function() {
        var displayNumberSelection = $("#displayNumberSelection");
        var displayNumber = displayNumberSelection.val();
        loadListPage(1, displayNumber);
    });

$(document).on("change",
    "#pageChangeSelect",
    function() {
        var displayNumberSelection = $("#displayNumberSelection");
        var pageChangeSelect = $("#pageChangeSelect");
        var displayNumber = displayNumberSelection.val();
        var pageNumber = pageChangeSelect.val();
        loadListPage(pageNumber, displayNumber);
    });