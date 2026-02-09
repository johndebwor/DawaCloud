# Client Company Logo Files

This directory contains the CLIENT company logo that appears on printable documents (invoices, receipts, reports).

**Important:** This is NOT the DawaCloud application logo. This is the logo of the company using the DawaCloud system (currently Revo Pharma & Medical Ltd).

## Logo Files

- **logo.svg** - PLACEHOLDER client company logo (replace with your actual company logo)
- This logo appears ONLY on printable documents and reports, not in the DawaCloud application interface

## Replacing the Logo

To use your actual company logo:

1. **For best results**, provide a high-quality PNG or SVG file
2. **Recommended specifications:**
   - **SVG**: Vector format, optimized for web
   - **PNG**: At least 400x400 pixels, transparent background
   - **Aspect ratio**: Square (1:1) or slightly rectangular
   - **File size**: Under 500KB for web performance

3. **File naming:**
   - Save your logo as `logo.svg` (preferred) or `logo.png`
   - Replace the placeholder files in this directory

4. **Where the client logo is used:**
   - Invoice headers (PDF documents)
   - Receipt printouts (thermal/A4 receipts)
   - Email notifications (sent on behalf of your company)
   - Reports and quotations (PDF documents)
   - **NOT used in the DawaCloud web application interface** (DawaCloud has its own branding)

## Logo Guidelines

- **Purpose**: This logo represents YOUR company (the client using DawaCloud)
- **Branding**: Use your company's official branding colors and design
- **Background**: Transparent or white background recommended
- **Content**: Your company logo (with or without company name)
- **Format**: Professional, clean, and legible at small sizes
- **Note**: No need to match DawaCloud's teal theme - use your own company colors

## Technical Notes

The InvoiceService uses QuestPDF which supports:
- SVG (recommended for quality)
- PNG/JPG (for raster images)
- Base64 embedded images

The logo is referenced in:
- `Features/Wholesale/Services/InvoiceService.cs` (for PDF invoices)
- `Features/Retail/Pages/POS.razor` (for thermal/A4 receipts)
- Email templates (when implemented)

## DawaCloud vs Client Branding

**DawaCloud Branding:**
- Application name: "DawaCloud"
- Application interface (header, navigation, etc.)
- Appears in the web application UI

**Client Company Branding (This Logo):**
- Your company name: e.g., "Revo Pharma & Medical Ltd"
- Printable documents (invoices, receipts, reports)
- Email communications sent to customers
- Appears ONLY on documents, not in the application interface
