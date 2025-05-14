// Actualizar el año actual en el footer
document.addEventListener('DOMContentLoaded', function() {
    const yearElements = document.querySelectorAll('#current-year');
    const currentYear = new Date().getFullYear();
    
    yearElements.forEach(element => {
        element.textContent = currentYear;
    });

    // Menú móvil
    const mobileMenuBtn = document.querySelector('.mobile-menu-btn');
    const navLinks = document.querySelector('.nav-links');
    
    if (mobileMenuBtn && navLinks) {
        mobileMenuBtn.addEventListener('click', function() {
            navLinks.classList.toggle('show');
        });
    }

    // Tabs en la página de envíos
    const tabBtns = document.querySelectorAll('.tab-btn');
    const tabContents = document.querySelectorAll('.tab-content');
    
    if (tabBtns.length > 0 && tabContents.length > 0) {
        tabBtns.forEach(btn => {
            btn.addEventListener('click', function() {
                const tabId = this.getAttribute('data-tab');
                
                // Desactivar todos los tabs
                tabBtns.forEach(btn => btn.classList.remove('active'));
                tabContents.forEach(content => content.classList.remove('active'));
                
                // Activar el tab seleccionado
                this.classList.add('active');
                document.getElementById(tabId).classList.add('active');
            });
        });
    }

    // Formulario de calculadora de envío
    const shippingForm = document.getElementById('shipping-calculator-form');
    if (shippingForm) {
        shippingForm.addEventListener('submit', function(e) {
            e.preventDefault();
            // Aquí iría la lógica para calcular el envío
            alert('Cálculo de envío realizado');
        });
    }

    // Formulario de login
    const loginForm = document.getElementById('login-form');
    if (loginForm) {
        loginForm.addEventListener('submit', function(e) {
            e.preventDefault();
            // Aquí iría la lógica para iniciar sesión
            alert('Inicio de sesión exitoso');
        });
    }

    // Formulario de registro
    const registerForm = document.getElementById('register-form');
    if (registerForm) {
        registerForm.addEventListener('submit', function(e) {
            e.preventDefault();
            
            const password = document.getElementById('password').value;
            const confirmPassword = document.getElementById('confirmPassword').value;
            
            if (password !== confirmPassword) {
                alert('Las contraseñas no coinciden');
                return;
            }
            
            // Aquí iría la lógica para registrar al usuario
            alert('Registro exitoso');
        });
    }

    // Formulario de recuperación de contraseña
    const recoverForm = document.getElementById('recover-form');
    if (recoverForm) {
        recoverForm.addEventListener('submit', function(e) {
            e.preventDefault();
            // Aquí iría la lógica para recuperar la contraseña
            alert('Se han enviado instrucciones a su correo electrónico');
        });
    }

    // Botones de agregar al carrito
    const addToCartBtns = document.querySelectorAll('.product-card .btn');
    
    if (addToCartBtns.length > 0) {
        addToCartBtns.forEach(btn => {
            btn.addEventListener('click', function() {
                const productName = this.closest('.product-card').querySelector('h3').textContent;
                alert(`${productName} agregado al carrito`);
            });
        });
    }
});

// Estilos adicionales para el menú móvil
document.head.insertAdjacentHTML('beforeend', `
    <style>
        @media (max-width: 767px) {
            .nav-links.show {
                display: flex;
                flex-direction: column;
                position: absolute;
                top: 100%;
                left: 0;
                right: 0;
                background-color: var(--primary-color);
                padding: 1rem;
                z-index: 100;
                box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
            }
            
            .nav-links.show a {
                margin: 0.5rem 0;
            }
        }
    </style>
`);