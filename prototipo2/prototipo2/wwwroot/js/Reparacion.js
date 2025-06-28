//Para el datatable de Reparaciones
document.addEventListener('DOMContentLoaded', function () {
    const datatablesSimple = document.getElementById('datatablesSimple');
    if (datatablesSimple) {
        new simpleDatatables.DataTable(datatablesSimple, {
            perPage: 10,
            labels: {
                placeholder: "Buscar reparación...",
                noRows: "No se encontraron reparaciones",
                info: "Mostrando {start} a {end} de {rows} reparaciones",
            },
            columns: [
                { select: 0, sort: "asc" }, // ID
                { select: 1, type: "date", format: "DD/MM/YYYY" }, // Fecha Ingreso
                { select: 2, type: "date", format: "DD/MM/YYYY" }  // Fecha Salida
            ]
        });
    }
});


