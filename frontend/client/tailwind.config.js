/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ['./src/**/*.{html,js}'], // Update this path based on your project structure
  theme: {
    extend: {
      // Custom Color Palette
      colors: {
        primary: '#5e60ce', // A modern blue for primary elements
        secondary: '#48bfe3', // A vibrant cyan for secondary elements
        accent: '#64dfdf', // A lighter shade for accents
        dark: '#1e1e1e', // Dark color for text or backgrounds
        light: '#f8f9fa', // Light color for backgrounds or text
        background: {
          default: '#F9FAFB', // Light gray for page background
          card: '#FFFFFF', // White for card backgrounds
          footer: '#111827', // Dark for the footer
        },
        text: {
          primary: '#111827', // Dark text for high contrast
          secondary: '#4B5563', // Lighter text for less emphasis
        },
      },

      // Custom Font Family
      fontFamily: {
        header: ['Poppins', 'sans-serif'],
        body: ['Roboto', 'sans-serif'],
      },

      // Custom Font Sizes
      fontSize: {
        base: '1rem',
        lg: '1.125rem',
        xl: '1.25rem',
        '2xl': '1.5rem',
        // Define more as needed
      },

      // Custom Border Styles
      borderColor: {
        default: '#D1D5DB', // Gray for light borders
        primary: '#5B21B6', // Primary color for distinct borders
      },
      borderWidth: {
        DEFAULT: '1px',
        0: '0',
        2: '2px',
        3: '3px',
        4: '4px',
        6: '6px',
        8: '8px',
      },
      borderRadius: {
        none: '0',
        sm: '0.125rem',
        DEFAULT: '0.25rem',
        md: '0.375rem',
        lg: '0.5rem',
        full: '9999px',
        // Define custom radii if needed
      },
    },
  },
  variants: {},
  plugins: [],
};
