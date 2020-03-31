ko.bindingHandlers.slideVisible = {
    update: function (element, valueAccessor, allBindings) {
        var value = valueAccessor();

        var valueUnwrapped = ko.unwrap(value);
        var duration = allBindings.get('slideDuration') || 200;

        if (valueUnwrapped == true)
            $(element).slideDown(duration);
        else
            $(element).slideUp(duration);
    }
};