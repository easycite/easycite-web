ko.bindingHandlers.element = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        let value = valueAccessor();
        
        value(element);
    }
};