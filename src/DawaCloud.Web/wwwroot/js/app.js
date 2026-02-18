// Theme Manager — Dark/Light mode toggle with localStorage persistence
window.themeManager = {
    isDark: function() {
        return document.documentElement.classList.contains('dark');
    },
    setTheme: function(isDark) {
        // Add transitioning class for smooth theme switch
        document.documentElement.classList.add('theme-transitioning');
        document.documentElement.classList.toggle('dark', isDark);
        localStorage.setItem('dawacloud-theme', isDark ? 'dark' : 'light');
        // Remove transitioning class after animation completes
        setTimeout(function() {
            document.documentElement.classList.remove('theme-transitioning');
        }, 350);
    },
    getPreference: function() {
        var saved = localStorage.getItem('dawacloud-theme');
        if (saved) return saved;
        return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    }
};

// DawaFlow JavaScript Functions

// Print functionality
window.printFunctions = {
    // Print the current page
    printPage: function () {
        window.print();
    },

    // Print a specific element by ID
    printElement: function (elementId) {
        const element = document.getElementById(elementId);
        if (!element) {
            console.error(`Element with ID '${elementId}' not found`);
            return;
        }

        // Create a new window for printing
        const printWindow = window.open('', '_blank', 'width=800,height=600');
        if (!printWindow) {
            console.error('Failed to open print window');
            return;
        }

        // Copy the element content
        printWindow.document.write(`
            <!DOCTYPE html>
            <html>
            <head>
                <title>Print</title>
                <style>
                    @media print {
                        body {
                            margin: 0;
                            padding: 20px;
                            font-family: Arial, sans-serif;
                        }
                        @page {
                            margin: 1cm;
                        }
                    }
                    body {
                        font-family: Arial, sans-serif;
                        padding: 20px;
                    }
                </style>
            </head>
            <body>
                ${element.innerHTML}
            </body>
            </html>
        `);

        printWindow.document.close();
        printWindow.focus();

        // Print after a short delay to ensure content is loaded
        setTimeout(() => {
            printWindow.print();
            printWindow.close();
        }, 250);
    },

    // Print receipt with custom HTML content
    printReceipt: function (receiptHtml) {
        const printWindow = window.open('', '_blank', 'width=300,height=600');
        if (!printWindow) {
            console.error('Failed to open print window');
            return;
        }

        printWindow.document.write(`
            <!DOCTYPE html>
            <html>
            <head>
                <title>Receipt</title>
                <style>
                    @media print {
                        body {
                            margin: 0;
                            padding: 10px;
                            font-family: 'Courier New', monospace;
                            font-size: 12px;
                        }
                        @page {
                            size: 80mm auto;
                            margin: 0;
                        }
                    }
                    body {
                        font-family: 'Courier New', monospace;
                        font-size: 12px;
                        width: 80mm;
                        padding: 10px;
                    }
                    .receipt-header {
                        text-align: center;
                        margin-bottom: 10px;
                    }
                    .receipt-divider {
                        border-top: 1px dashed #000;
                        margin: 5px 0;
                    }
                    .receipt-item {
                        display: flex;
                        justify-content: space-between;
                        margin: 3px 0;
                    }
                    .receipt-total {
                        font-weight: bold;
                        font-size: 14px;
                    }
                </style>
            </head>
            <body>
                ${receiptHtml}
            </body>
            </html>
        `);

        printWindow.document.close();
        printWindow.focus();

        setTimeout(() => {
            printWindow.print();
            printWindow.close();
        }, 250);
    },

    // Print invoice (A4 format)
    printInvoice: function (invoiceHtml) {
        const printWindow = window.open('', '_blank', 'width=800,height=1100');
        if (!printWindow) {
            console.error('Failed to open print window');
            return;
        }

        printWindow.document.write(`
            <!DOCTYPE html>
            <html>
            <head>
                <title>Invoice</title>
                <style>
                    @media print {
                        body {
                            margin: 0;
                            padding: 0;
                        }
                        @page {
                            size: A4;
                            margin: 1.5cm;
                        }
                    }
                    body {
                        font-family: Arial, sans-serif;
                        font-size: 11pt;
                        line-height: 1.4;
                        padding: 20px;
                    }
                    .invoice-header {
                        text-align: center;
                        margin-bottom: 30px;
                    }
                    .invoice-details {
                        margin-bottom: 20px;
                    }
                    table {
                        width: 100%;
                        border-collapse: collapse;
                        margin: 20px 0;
                    }
                    th, td {
                        border: 1px solid #ddd;
                        padding: 8px;
                        text-align: left;
                    }
                    th {
                        background-color: #f2f2f2;
                    }
                    .invoice-total {
                        text-align: right;
                        margin-top: 20px;
                    }
                </style>
            </head>
            <body>
                ${invoiceHtml}
            </body>
            </html>
        `);

        printWindow.document.close();
        printWindow.focus();

        setTimeout(() => {
            printWindow.print();
            printWindow.close();
        }, 250);
    }
};

// File download functionality
window.downloadFile = function (fileName, base64Content, contentType) {
    try {
        // Convert base64 to blob
        const byteCharacters = atob(base64Content);
        const byteNumbers = new Array(byteCharacters.length);
        for (let i = 0; i < byteCharacters.length; i++) {
            byteNumbers[i] = byteCharacters.charCodeAt(i);
        }
        const byteArray = new Uint8Array(byteNumbers);
        const blob = new Blob([byteArray], { type: contentType });

        // Create download link
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = fileName;

        // Trigger download
        document.body.appendChild(link);
        link.click();

        // Cleanup
        document.body.removeChild(link);
        window.URL.revokeObjectURL(url);
    } catch (error) {
        console.error('Error downloading file:', error);
        alert('Failed to download file. Please try again.');
    }
};

// Scroll to element by ID
window.scrollToElement = function (elementId) {
    var element = document.getElementById(elementId);
    if (element) {
        setTimeout(function () {
            element.scrollIntoView({ behavior: 'smooth', block: 'start' });
        }, 100);
    }
};

// Keyboard Shortcuts
window.keyboardShortcuts = {
    // Initialize keyboard shortcuts
    initialize: function (dotNetHelper) {
        // Store reference to .NET helper
        window._dotNetHelper = dotNetHelper;

        // Add keyboard event listener
        document.addEventListener('keydown', function (e) {
            // Get current page path
            const path = window.location.pathname.toLowerCase();

            // Ctrl+K - Command Palette (global)
            if (e.ctrlKey && e.key === 'k') {
                e.preventDefault();
                console.log('Ctrl+K pressed - Command Palette');
                // TODO: Implement command palette
                alert('Command Palette (Ctrl+K) - Coming soon!');
                return;
            }

            // Escape - Close dialogs (global)
            if (e.key === 'Escape') {
                // Let MudBlazor handle this natively
                return;
            }

            // POS-specific shortcuts
            if (path.includes('/pos') || path.includes('/retail')) {
                // F2 - Focus search
                if (e.key === 'F2') {
                    e.preventDefault();
                    const searchInput = document.querySelector('input[placeholder*="Search"], input[placeholder*="search"], input[type="search"]');
                    if (searchInput) {
                        searchInput.focus();
                        searchInput.select();
                    }
                    return;
                }

                // F4 - Hold sale
                if (e.key === 'F4') {
                    e.preventDefault();
                    console.log('F4 pressed - Hold Sale');
                    // Trigger hold sale button click
                    const holdButton = document.querySelector('button[title*="Hold"], button:has(> svg.mud-icon-root)');
                    if (holdButton && holdButton.textContent.toLowerCase().includes('hold')) {
                        holdButton.click();
                    }
                    return;
                }

                // F5 - Recall sale
                if (e.key === 'F5') {
                    e.preventDefault();
                    console.log('F5 pressed - Recall Sale');
                    // Trigger recall button click
                    const recallButton = Array.from(document.querySelectorAll('button')).find(btn =>
                        btn.textContent.toLowerCase().includes('recall')
                    );
                    if (recallButton) {
                        recallButton.click();
                    }
                    return;
                }

                // F12 - Process payment
                if (e.key === 'F12') {
                    e.preventDefault();
                    console.log('F12 pressed - Process Payment');
                    // Trigger pay button click
                    const payButton = Array.from(document.querySelectorAll('button')).find(btn =>
                        btn.textContent.toLowerCase().includes('pay now') ||
                        btn.textContent.toLowerCase().includes('process payment')
                    );
                    if (payButton) {
                        payButton.click();
                    }
                    return;
                }
            }

            // Drug list page shortcuts
            if (path.includes('/drugs') && !path.includes('/new') && !path.includes('/edit')) {
                // Ctrl+N - New drug
                if (e.ctrlKey && e.key === 'n') {
                    e.preventDefault();
                    window.location.href = '/drugs/new';
                    return;
                }
            }

            // Form pages - Ctrl+S to save
            if (path.includes('/new') || path.includes('/edit') || path.includes('/form')) {
                if (e.ctrlKey && e.key === 's') {
                    e.preventDefault();
                    console.log('Ctrl+S pressed - Save Form');
                    // Find and click the primary save button
                    const saveButton = Array.from(document.querySelectorAll('button')).find(btn =>
                        (btn.textContent.toLowerCase().includes('save') ||
                            btn.textContent.toLowerCase().includes('submit') ||
                            btn.textContent.toLowerCase().includes('create')) &&
                        btn.classList.contains('mud-button-filled')
                    );
                    if (saveButton) {
                        saveButton.click();
                    }
                    return;
                }
            }
        });

        console.log('Keyboard shortcuts initialized');
    },

    // Dispose keyboard shortcuts
    dispose: function () {
        if (window._dotNetHelper) {
            window._dotNetHelper = null;
        }
    }
};

// Focus management
window.focusElement = function (elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.focus();
        if (element.select) {
            element.select();
        }
    }
};
