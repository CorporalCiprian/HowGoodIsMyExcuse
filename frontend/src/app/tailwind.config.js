/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ['./src/**/*.{html,ts}'],
  theme: {
    extend: {
      colors: {
        surface:                   '#131313',
        'surface-lowest':          '#0d0d0d',
        'surface-low':             '#1c1b1b',
        'surface-mid':             '#232323',
        'surface-high':            '#2a2a2a',
        'surface-highest':         '#353534',
        'surface-bright':          '#303030',
        primary:                   '#b9c9d3',
        'primary-container':       '#2f3e46',
        secondary:                 '#7dffa2',
        'secondary-container':     '#05e777',
        'on-secondary':            '#003918',
        tertiary:                  '#ffb4a2',
        'tertiary-container':      '#761700',
        'on-tertiary':             '#621100',
        'on-surface':              '#e5e2e1',
        'on-surface-variant':      '#a0a5a8',
        'outline-variant':         'rgba(67,71,74,0.15)',
      },
      fontFamily: {
        display: ['"Space Grotesk"', 'sans-serif'],
        body:    ['"Inter"', 'sans-serif'],
      },
      fontSize: {
        'label-sm': ['0.6875rem', { letterSpacing: '0.1em', fontWeight: '600' }],
      },
      borderRadius: {
        md: '0.375rem',
        lg: '0.5rem',
      },
      boxShadow: {
        'ambient': '0 12px 40px rgba(0,0,0,0.4)',
        'ambient-lg': '0 16px 48px rgba(0,0,0,0.5)',
      },
      backgroundImage: {
        'primary-gradient': 'linear-gradient(135deg, #b9c9d3 0%, #2f3e46 100%)',
      },
      backdropBlur: {
        glass: '16px',
      },
    },
  },
  plugins: [],
};