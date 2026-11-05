// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('[data-format]').forEach((input) => {
        const formatear = () => {
            const digitos = input.value.replace(/\D/g, '');

            if (input.dataset.format === 'telefono') {
                const valor = digitos.slice(0, 8);
                input.value = valor.length > 4
                    ? `${valor.slice(0, 4)}-${valor.slice(4)}`
                    : valor;
            } else if (input.dataset.format === 'dui') {
                const valor = digitos.slice(0, 9);
                input.value = valor.length > 8
                    ? `${valor.slice(0, 8)}-${valor.slice(8)}`
                    : valor;
            }
        };

        input.addEventListener('input', formatear);
        input.addEventListener('blur', formatear);
    });
});
