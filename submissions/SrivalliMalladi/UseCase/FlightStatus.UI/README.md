# FlightStatus UI

This project is an Angular 21 application that is hosted with a small Express server for production deployment.

## Prerequisites

- Node.js 22.x or later
- npm 11.x or later
- Azure App Service (Linux) with Node.js runtime

## Local development and production run

### Install dependencies

From the project root (`FlightStatus.UI`):

```bash
npm install
```

### Build for production

```bash
npm run build
```

This generates the production build files in:

```text
dist/FlightStatus.UI/browser
```

### Start the local server

```bash
npm start
```

Then open your browser at:

```text
http://localhost:8080
```

### What `npm start` does

The `package.json` script runs `node server.js`, which serves the static build output from `dist/FlightStatus.UI/browser` and returns `index.html` for all routes.

## Azure App Service deployment

### Folder to deploy

Deploy the entire `FlightStatus.UI` folder, not only the `dist` folder. The deployed folder must include:

- `package.json`
- `server.js`
- `angular.json`
- `src/`
- `dist/FlightStatus.UI/browser`

### Azure App Service settings

- OS: **Linux**
- Runtime stack: **Node 22 LTS**
- Startup command: `npm start`

### Verify deployment

After deployment completes and the app restarts, open the app URL and verify that the Angular app loads.

If the app does not load:

1. Confirm `npm start` is set in App Service `Configuration > General settings`
2. Confirm `server.js` exists in the deployed root
3. Confirm `dist/FlightStatus.UI/browser` exists after build
4. Check App Service logs for startup or server errors

### Cloud links

- Application URL: _update this with your Azure App Service URL_
- Azure deployment portal: _update this with your Azure App Service link_

## Code scaffolding

Angular CLI includes powerful code scaffolding tools. To generate a new component, run:

```bash
ng generate component component-name
```

For a complete list of available schematics (such as `components`, `directives`, or `pipes`), run:

```bash
ng generate --help
```

## Building

To build the project run:

```bash
ng build
```

This will compile your project and store the build artifacts in the `dist/` directory. By default, the production build optimizes your application for performance and speed.

## Running unit tests

To execute unit tests with the [Vitest](https://vitest.dev/) test runner, use the following command:

```bash
ng test
```

## Running end-to-end tests

For end-to-end (e2e) testing, run:

```bash
ng e2e
```

Angular CLI does not come with an end-to-end testing framework by default. You can choose one that suits your needs.

## Additional Resources

For more information on using the Angular CLI, including detailed command references, visit the [Angular CLI Overview and Command Reference](https://angular.dev/tools/cli) page.
