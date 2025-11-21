# Header/Navbar Quick Reference

## ?? Key Features

### Desktop View (?992px)
```
???????????????????????????????????????????????????????????????
? [?? InvenEdu]  [Dashboard] [Inventory] ... [?? User] [?] [??] ?
???????????????????????????????????????????????????????????????
```

### Mobile View (?991px)
```
???????????????????????????????
? [?? InvenEdu]          [?]  ?
???????????????????????????????
        ? (Tap hamburger)
???????????????????????????????
? [?? InvenEdu]          [?]  ?
???????????????????????????????
? [Dashboard]                 ?
? [Inventory]                 ?
? [Issuances]                 ?
? [Categories]                ?
? [Users]                     ?
???????????????????????????????
? [?? John Doe]              ?
? [? Logout]                 ?
? [?? Theme]                 ?
???????????????????????????????
```

---

## ?? Visual States

### Navigation Links

| State | Appearance |
|-------|-----------|
| **Default** | Gray text, no background |
| **Hover** | Light background, primary color text |
| **Active** | Primary color background, white text |
| **Focus** | 2px outline, primary color |

### Buttons

| Button | Desktop | Mobile |
|--------|---------|--------|
| **Logout** | Icon + "Logout" | Icon only |
| **Theme** | Icon only | Icon only |

---

## ?? Responsive Breakpoints

```css
/* Large Desktop */
@media (min-width: 1200px) { }

/* Desktop */
@media (min-width: 992px) { 
    /* Horizontal navbar */
}

/* Tablet */
@media (max-width: 991px) { 
    /* Show hamburger menu */
}

/* Mobile */
@media (max-width: 768px) { 
    /* Compact header */
}

/* Small Mobile */
@media (max-width: 480px) { 
    /* Hide text labels */
}
```

---

## ?? Common Customizations

### Change Navbar Height
```css
:root {
    --navbar-height: 70px;  /* Change this value */
}
```

### Change Primary Color
```css
:root {
    --primary-color: #3674B5;  /* Your brand color */
}
```

### Adjust Mobile Breakpoint
```css
@media (max-width: 991px) {  /* Change 991px to your preferred breakpoint */
    .navbar-toggler { display: block; }
}
```

---

## ?? Quick Tips

### Adding New Navigation Link

**Admin Menu:**
```razor
<li class="nav-item">
    <a class="nav-link @(ViewContext.RouteData.Values["Action"]?.ToString() == "YourAction" ? "active" : "")" 
       asp-controller="Admin" 
       asp-action="YourAction">
        <i class="fas fa-your-icon"></i>
        <span>Your Label</span>
    </a>
</li>
```

**User Menu:**
```razor
<li class="nav-item">
    <a class="nav-link @(ViewContext.RouteData.Values["Action"]?.ToString() == "YourAction" ? "active" : "")" 
       asp-controller="User" 
       asp-action="YourAction">
        <i class="fas fa-your-icon"></i>
        <span>Your Label</span>
    </a>
</li>
```

### Adding Dropdown Menu (Future Enhancement)
```html
<li class="nav-item dropdown">
    <a class="nav-link dropdown-toggle" href="#" id="navbarDropdown">
        <i class="fas fa-cog"></i> Settings
    </a>
    <ul class="dropdown-menu">
        <li><a class="dropdown-item" href="#">Option 1</a></li>
        <li><a class="dropdown-item" href="#">Option 2</a></li>
    </ul>
</li>
```

---

## ?? Troubleshooting

### Menu Not Toggling
**Check:**
- JavaScript loaded: `site.js`
- IDs match: `navbarToggler` and `navbarCollapse`
- Console for errors

### Active State Not Working
**Check:**
- Route values match exactly
- Case sensitivity
- Controller/Action names

### Theme Not Persisting
**Check:**
- localStorage available
- JavaScript enabled
- Browser privacy settings

### Mobile Menu Staying Open
**Check:**
- Click outside handler running
- No JavaScript errors
- CSS transitions working

---

## ?? Checklist for New Pages

When creating a new page, ensure:

- [ ] Navigation link added to appropriate section (Admin/User)
- [ ] Active state logic implemented
- [ ] Icon selected from Font Awesome
- [ ] Label is clear and concise
- [ ] Mobile view tested
- [ ] Active highlighting works
- [ ] Keyboard navigation tested
- [ ] Theme compatibility verified

---

## ?? Best Practices

### DO ?
- Keep labels short and clear
- Use semantic HTML
- Test on multiple devices
- Maintain consistent icon style
- Provide accessible labels
- Use proper ARIA attributes

### DON'T ?
- Add too many top-level links (max 5-7)
- Use similar icons for different actions
- Forget mobile testing
- Remove ARIA labels
- Hardcode colors (use CSS variables)
- Skip accessibility testing

---

## ?? Resources

### Font Awesome Icons
- [Font Awesome Library](https://fontawesome.com/icons)
- Common icons:
  - Dashboard: `fa-tachometer-alt`
  - Inventory: `fa-boxes`
  - Users: `fa-users`
  - Settings: `fa-cog`
  - Search: `fa-search`
  - Add: `fa-plus`
  - Edit: `fa-edit`
  - Delete: `fa-trash`

### Color Variables
```css
--primary-color: #3674B5
--success-color: #28A745
--danger-color: #DC3545
--warning-color: #FFC107
--info-color: #17A2B8
```

---

## ?? Performance Tips

1. **Minimize Reflows**: Use CSS transforms instead of position changes
2. **Debounce Scroll**: Already implemented for navbar scroll effect
3. **Use CSS Variables**: Fast theme switching without re-parsing
4. **Lazy Load Icons**: Only load icons that are used
5. **Optimize Images**: Use SVG for brand logos

---

## ?? Support

For issues or questions:
1. Check browser console for errors
2. Verify all files are loaded correctly
3. Test in incognito mode (no extensions)
4. Check responsive design mode
5. Review HEADER_IMPROVEMENTS.md for detailed documentation

---

**Last Updated**: Based on current implementation
**Version**: 1.0
**Compatibility**: .NET 9, Bootstrap 5, Font Awesome 6
