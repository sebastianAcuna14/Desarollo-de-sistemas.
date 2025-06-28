document.addEventListener('DOMContentLoaded', function () {
    const datatablesSimple = document.getElementById('datatablesSimple');
    if (datatablesSimple) {
        new simpleDatatables.DataTable(datatablesSimple, {
            perPage: 10,
            labels: {
                placeholder: "Buscar categoría...",
                noRows: "No se encontraron categorías",
                info: "Mostrando {start} a {end} de {rows} categorías"
            },
            columns: [
                { select: 0, sort: "asc" }
            ]
        });
    }
});



