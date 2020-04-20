ko.bindingHandlers.autoBsTooltip = {
    update: function (element, valueAccessor, allBindings) {
        const value = valueAccessor();
        const valueUnwrapped = ko.unwrap(value);

        const placement = allBindings.get('tooltipPlacement') || 'top';

        $(element).tooltip('dispose').tooltip({
            placement: placement,
            title: valueUnwrapped
        });
    }
};