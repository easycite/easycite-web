ko.bindingHandlers.loader = {
    loader: null,
    init: function(element, valueAccessor, allBindings, viewModel, bindingContext) {
        loader = $(`<div class="loader d-flex justify-content-center align-items-center">
            <div class="spinner-border text-primary" role="status">
                <span class="sr-only">Loading...</span>
            </div>
        </div>`);
        loader.appendTo(element);
    },
    update: function(element, valueAccessor, allBindings, viewModel, bindingContext) {
        let isLoading = ko.unwrap(valueAccessor());
        
        if(isLoading === true)
            loader.removeClass('invisible');
        else
            loader.addClass('invisible');
    }
};