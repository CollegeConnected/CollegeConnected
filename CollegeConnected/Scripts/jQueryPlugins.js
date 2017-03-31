// From http://javaevangelist.blogspot.com/2013/01/internet-explorer-9-ie9-table-white.html

jQuery.fn.htmlClean = function() {
    this.contents().filter(function() {
        if (this.nodeType != 3) {
            $(this).htmlClean();
            return false;
        } else {
            return !/\S/.test(this.nodeValue);
        }
    }).remove();
    return this;
};

function fixIE9Bug() {
    $(".ListTable").htmlClean();
}

jQuery.fn.accountNumberSelectBox = function(initValue) {
    if ((typeof initValue !== "undefined") && (typeof initValue.id !== "undefined"))
        this.val(initValue.id);
    this.select2({
        placeholder: "Select an Account",
        minimumInputLength: 0,
        allowClear: true,
        width: "600px",
        initSelection: function(element, callback) {
            if ((typeof initValue !== "undefined") && (typeof initValue.id !== "undefined"))
                callback(initValue);
        },
        ajax: {
            quietMillis: 150, //TODO: Magic Number
            url: "GetAccounts",
            dataType: "json",
            data: function(term, page) {
                return {
                    pageSize: 100, //TODO: Magic Number
                    pageNum: page,
                    searchTerm: term
                };
            },
            results: function(data, page) {
                var more = page * 100 < data.Total;
                return { results: data.Results, more: more };
            }
        }
    });
};
jQuery.fn.mappingSelectBox = function() {
    var initValue = $(this).closest("tr").data("initval");
    this.select2({
        placeholder: "Select a Mapping",
        minimumInputLength: 0,
        allowClear: true,
        width: "300px",
        initSelection: function(element, callback) {
            if ((typeof initValue !== "undefined") && (typeof initValue.id !== "undefined"))
                callback(initValue);
        },
        ajax: {
            quietMillis: 150, //TODO: Magic Number
            url: "GetMappings",
            dataType: "json",
            data: function(term, page) {
                return {
                    categoryID: $(this).data("categoryID"),
                    pageSize: 100, //TODO: Magic Number
                    pageNum: page,
                    searchTerm: term
                };
            },
            results: function(data, page) {
                var more = page * 100 < data.Total;
                return { results: data.Results, more: more };
            }
        }
    });
};