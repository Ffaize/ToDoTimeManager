function initializeOtpInputs(containerId) {
    const container = document.getElementById(containerId);
    if (!container) return;
    const inputs = Array.from(container.querySelectorAll('.otp-cell'));

    inputs.forEach((input, index) => {
        input.addEventListener('keydown', function (e) {
            if (e.key === 'Backspace' && this.value === '') {
                e.preventDefault();
                if (index > 0) inputs[index - 1].focus();
            }
        });
        input.addEventListener('input', function () {
            if (this.value.length > 0 && index < inputs.length - 1) {
                inputs[index + 1].focus();
            }
        });
    });

    inputs[0]?.focus();
}

function initializeEmbers() {
    const container = document.getElementById('embers');
    if (!container) return;
    container.innerHTML = '';
    const count = 24;
    for (let i = 0; i < count; i++) {
        const e = document.createElement('div');
        e.className = 'ember';
        e.style.left = Math.random() * 100 + '%';
        e.style.animationDuration = 8 + Math.random() * 10 + 's';
        e.style.animationDelay = Math.random() * 12 + 's';
        e.style.setProperty('--drift', Math.random() * 80 - 40 + 'px');
        e.style.width = e.style.height = 1 + Math.random() * 3 + 'px';
        container.appendChild(e);
    }
}
