ko.bindingHandlers.loader = {
    init: function(element, valueAccessor, allBindings, viewModel, bindingContext) {
        element.loader = $(`<div class="loader d-flex justify-content-center align-items-center">
            <div class="spinner-border text-primary" role="status">
                <span class="sr-only">Loading...</span>
            </div>
        </div>`);
        element.loader.appendTo(element);
    },
    update: function(element, valueAccessor, allBindings, viewModel, bindingContext) {
        let isLoading = ko.unwrap(valueAccessor());
        
        if(isLoading === true)
            $(element).children('.loader').removeAttr('hidden');
        else
            $(element).children('.loader').attr('hidden', true);
    }
};

ko.bindingHandlers.inlineLoader = {
    init: function(element, valueAccessor, allBindings, viewModel, bindingContext) {
        let loader = $(`<div class="loader inline-loader d-flex justify-content-center align-items-center">
            <div class="spinner-border text-primary" role="status">
                <span class="sr-only">Loading...</span>
            </div>
        </div>`);
        loader.appendTo(element);
    },
    update: function(element, valueAccessor, allBindings, viewModel, bindingContext) {
        let isLoading = ko.unwrap(valueAccessor());
        
        if(isLoading === true)
            $(element).children('.loader').removeAttr('hidden');
        else
            $(element).children('.loader').attr('hidden', true);
    }
};