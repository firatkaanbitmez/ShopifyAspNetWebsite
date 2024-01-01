document.addEventListener('DOMContentLoaded', function () {
    // Sepet güncellemesi için AJAX çağrısı
    function updateCartItemCount() {
        fetch('/Cart/GetCartItemCount')
            .then(response => response.text())
            .then(count => {
                document.getElementById('cartItemCount').innerText = count;
            })
            .catch(error => console.error('Error:', error));
    }

    updateCartItemCount();

    // Arama çubuğu animasyonu
    const searchBar = document.querySelector('.search-bar input[type="search"]');
    searchBar.addEventListener('focus', () => {
        searchBar.style.boxShadow = '0 0 10px rgba(0,0,0,.5)';

    });
    searchBar.addEventListener('blur', () => {
        searchBar.style.boxShadow = 'none';
    });
});
