// Image crop interop for Cropper.js
window.imageCrop = {
    _cropper: null,

    init: function (imageElementId, aspectRatio) {
        var self = this;
        var img = document.getElementById(imageElementId);
        if (!img) return;

        // Destroy existing instance
        if (self._cropper) {
            self._cropper.destroy();
            self._cropper = null;
        }

        function createCropper() {
            self._cropper = new Cropper(img, {
                aspectRatio: aspectRatio || 1,
                viewMode: 1,
                dragMode: 'move',
                autoCropArea: 0.9,
                responsive: true,
                restore: false,
                guides: true,
                center: true,
                highlight: false,
                cropBoxMovable: true,
                cropBoxResizable: true,
                toggleDragModeOnDblclick: false,
                minContainerWidth: 300,
                minContainerHeight: 300
            });
        }

        // Wait for image to fully load before initializing Cropper
        if (img.complete && img.naturalWidth > 0) {
            createCropper();
        } else {
            img.onload = function () {
                createCropper();
            };
            img.onerror = function () {
                console.error('Image failed to load for cropper:', img.src);
            };
        }
    },

    getCroppedImage: function (width, height, mimeType) {
        if (!this._cropper) return null;

        var canvas = this._cropper.getCroppedCanvas({
            width: width || 200,
            height: height || 200,
            imageSmoothingEnabled: true,
            imageSmoothingQuality: 'high'
        });

        if (!canvas) return null;
        return canvas.toDataURL(mimeType || 'image/png');
    },

    destroy: function () {
        if (this._cropper) {
            this._cropper.destroy();
            this._cropper = null;
        }
    }
};
