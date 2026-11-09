import { vars } from 'nativewind';

// Raw color values - update these and they sync everywhere
export const colors = {
  light: {
    '--card': '255 255 255',
    '--card-foreground': '56 56 56',
    '--popover': '255 255 255',
    '--popover-foreground': '56 56 56',

    '--background': '255 255 255',
    '--background-secondary': '248 248 251',
    '--background-muted': '233 233 233',
    '--foreground': '56 56 56',
    '--foreground-secondary': '109 109 111',
    '--foreground-muted': '167 167 170',

    '--primary': '240 91 37',
    '--primary-foreground': '255 255 255',
    '--secondary': '233 233 233',
    '--secondary-foreground': '56 56 56',
    '--muted': '233 233 233',
    '--muted-foreground': '167 167 170',
    '--accent': '247 247 247',
    '--accent-foreground': '52 52 52',
    '--destructive': '231 0 11',
    '--destructive-foreground': '255 255 255',

    '--border': '244 244 244',
    '--input': '239 239 244',
    '--ring': '240 91 37',

    '--info': '43 117 238',
    '--error': '221 60 60',
    '--success': '75 179 75',
    '--warning': '245 166 35',

    '--chart-1': '240 91 37',
    '--chart-2': '43 117 238',
    '--chart-3': '75 179 75',
    '--chart-4': '245 166 35',
    '--chart-5': '155 89 182',

    '--sidebar': '248 248 251',
    '--sidebar-foreground': '56 56 56',
    '--sidebar-primary': '240 91 37',
    '--sidebar-primary-foreground': '255 255 255',
    '--sidebar-accent': '233 233 233',
    '--sidebar-accent-foreground': '56 56 56',
    '--sidebar-border': '244 244 244',
    '--sidebar-ring': '240 91 37',
  },
  dark: {
    '--card': '39 39 39',
    '--card-foreground': '238 238 238',
    '--popover': '39 39 39',
    '--popover-foreground': '238 238 238',

    '--background': '32 32 32',
    '--background-secondary': '39 39 39',
    '--background-muted': '46 46 46',
    '--foreground': '238 238 238',
    '--foreground-secondary': '169 169 169',
    '--foreground-muted': '118 118 118',

    '--primary': '240 91 37',
    '--primary-foreground': '255 255 255',
    '--secondary': '53 53 53',
    '--secondary-foreground': '238 238 238',
    '--muted': '46 46 46',
    '--muted-foreground': '169 169 169',
    '--accent': '53 53 53',
    '--accent-foreground': '250 250 250',
    '--destructive': '255 100 103',
    '--destructive-foreground': '255 255 255',

    '--border': '43 43 43',
    '--input': '46 46 46',
    '--ring': '240 91 37',

    '--info': '43 117 238',
    '--error': '221 60 60',
    '--success': '75 179 75',
    '--warning': '245 166 35',

    '--chart-1': '255 124 77',
    '--chart-2': '91 155 255',
    '--chart-3': '107 203 107',
    '--chart-4': '255 190 80',
    '--chart-5': '190 126 218',

    '--sidebar': '39 39 39',
    '--sidebar-foreground': '238 238 238',
    '--sidebar-primary': '240 91 37',
    '--sidebar-primary-foreground': '255 255 255',
    '--sidebar-accent': '53 53 53',
    '--sidebar-accent-foreground': '238 238 238',
    '--sidebar-border': '43 43 43',
    '--sidebar-ring': '240 91 37',
  },
};

// Config for nativewind vars() - used by provider
export const config = {
  light: vars(colors.light),
  dark: vars(colors.dark),
};
