ko.bindingHandlers['visibleAttr'] = {
    'update': function (element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        var isCurrentlyVisible = !element.hasAttribute('hidden');
        if (value && !isCurrentlyVisible)
            element.removeAttribute('hidden');
        else if ((!value) && isCurrentlyVisible)
            element.setAttribute('hidden', '');
    }
};

ko.bindingHandlers['hiddenAttr'] = {
    'update': function (element, valueAccessor) {
        ko.bindingHandlers['visible']['update'](element, function() { return !ko.utils.unwrapObservable(valueAccessor()) });
    }
};