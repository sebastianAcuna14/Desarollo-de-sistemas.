document.addEventListener('DOMContentLoaded', function () {
    const datatablesSimple = document.getElementById('datatablesSimple');
    if (datatablesSimple) {
        new simpleDatatables.DataTable(datatablesSimple, {
            perPage: 10,
            labels: {
                placeholder: "Buscar proveedor...",
                noRows: "No se encontraron proveedores",
                info: "Mostrando {start} a {end} de {rows} proveedores",
            }
        });
    }
});
