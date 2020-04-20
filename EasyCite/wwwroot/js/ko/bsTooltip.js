ko.bindingHandlers.autoBsTooltip = {
    update: function (element, valueAccessor, allBindings) {
        const value = valueAccessor();
        const valueUnwrapped = ko.utils.unwrapObservable(value);

        const placement = allBindings.get('tooltipPlacement') || 'top';

        if(valueUnwrapped !== undefined
            && valueUnwrapped !== null
            && valueUnwrapped !== '')
            $(element).tooltip({
                placement: placement,
                title: value,
                trigger: 'hover'
            });
        else
            $(element).tooltip('dispose');

        if(element == document.activeElement)
            $(element).tooltip('show');
    }
};