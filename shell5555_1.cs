#please provide a c program to create a linux shell program includes few commands: 
#whoami, uname, pwd, ls -la, ps -ef, cd and get the directory to change as input from user, mkdir and get name as input, 
#delete file or directory get input, detect mouse click- if the user press left print left and if right print right, 
#write command and output to log, please print log before exit, each command is running please print which system call is used and from what library
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <sys/stat.h>
#include <sys/types.h>
#include <dirent.h>
#include <X11/Xlib.h>
#include <X11/Xutil.h>
#include <X11/Xos.h>

#define LOG_FILE "command_log.txt"

void log_command(const char *command, const char *output) {
    FILE *log = fopen(LOG_FILE, "a");
    if (log == NULL) {
        perror("Failed to open log file");
        return;
    }
    fprintf(log, "Command: %s\nOutput:\n%s\n\n", command, output);
    fclose(log);
}

void execute_command(const char *command) {
    printf("Executing system command: %s\n", command);
    printf("System call: system() from stdlib.h\n\n");

    FILE *fp;
    char output[1024] = "";
    fp = popen(command, "r");
    if (fp == NULL) {
        perror("Failed to run command");
        return;
    }

    while (fgets(output, sizeof(output) - 1, fp) != NULL) {
        printf("%s", output);
    }

    log_command(command, output);
    pclose(fp);
}

void change_directory(const char *path) {
    printf("Changing directory to: %s\n", path);
    printf("System call: chdir() from unistd.h\n\n");

    if (chdir(path) != 0) {
        perror("Failed to change directory");
    } else {
        printf("Successfully changed directory to %s\n", path);
    }
}

void create_directory(const char *dir_name) {
    printf("Creating directory: %s\n", dir_name);
    printf("System call: mkdir() from sys/stat.h\n\n");

    if (mkdir(dir_name, 0755) != 0) {
        perror("Failed to create directory");
    } else {
        printf("Directory %s created successfully.\n", dir_name);
    }
}

void delete_file_or_directory(const char *name) {
    printf("Deleting file/directory: %s\n", name);
    printf("System call: unlink() or rmdir() from unistd.h\n\n");

    if (unlink(name) == 0) {
        printf("File %s deleted successfully.\n", name);
    } else if (rmdir(name) == 0) {
        printf("Directory %s deleted successfully.\n", name);
    } else {
        perror("Failed to delete file or directory");
    }
}

void detect_mouse_click() {
    Display *display = XOpenDisplay(NULL);
    if (display == NULL) {
        fprintf(stderr, "Cannot open display\n");
        return;
    }

    int screen = DefaultScreen(display);
    Window root = RootWindow(display, screen);

    // Create a simple window
    Window win = XCreateSimpleWindow(display, root, 10, 10, 200, 100, 1,
                                     BlackPixel(display, screen), WhitePixel(display, screen));

    // Select input events for the window
    XSelectInput(display, win, ButtonPressMask);

    // Map the window to display it
    XMapWindow(display, win);

    XEvent event;
    printf("Waiting for mouse click inside the created window...\n");

    while (1) {
        XNextEvent(display, &event);
        if (event.type == ButtonPress) {
            if (event.xbutton.button == Button1) {
                printf("Left mouse button clicked\n");
                break;
            } else if (event.xbutton.button == Button3) {
                printf("Right mouse button clicked\n");
                break;
            }
        }
    }

    XDestroyWindow(display, win);
    XCloseDisplay(display);
}

void print_log() {
    printf("\n--- Command Log ---\n");

    FILE *log = fopen(LOG_FILE, "r");
    if (log == NULL) {
        perror("Failed to open log file");
        return;
    }

    char line[256];
    while (fgets(line, sizeof(line), log) != NULL) {
        printf("%s", line);
    }

    fclose(log);
}

void display_menu() {
    printf("\n--- Simple Shell Menu ---\n");
    printf("1. whoami - Display current user\n");
    printf("2. uname - Display system information\n");
    printf("3. pwd - Print current working directory\n");
    printf("4. ls -la - List directory contents\n");
    printf("5. ps -ef - Display process list\n");
    printf("6. cd - Change directory\n");
    printf("7. mkdir - Create a new directory\n");
    printf("8. delete - Delete a file or directory\n");
    printf("9. mouse - Detect mouse click\n");
    printf("10. exit - Exit the shell\n");
    printf("---------------------------\n");
}

int main() {
    char input[256];
    int choice;

    while (1) {
        display_menu();
        printf("\nEnter the number of your choice: ");
        fgets(input, sizeof(input), stdin);
        choice = atoi(input);

        switch (choice) {
            case 1:
                execute_command("whoami");
                break;
            case 2:
                execute_command("uname");
                break;
            case 3:
                execute_command("pwd");
                break;
            case 4:
                execute_command("ls -la");
                break;
            case 5:
                execute_command("ps -ef");
                break;
            case 6:
                printf("Enter the directory to change to: ");
                fgets(input, sizeof(input), stdin);
                input[strcspn(input, "\n")] = 0;
                change_directory(input);
                break;
            case 7:
                printf("Enter the name of the directory to create: ");
                fgets(input, sizeof(input), stdin);
                input[strcspn(input, "\n")] = 0;
                create_directory(input);
                break;
            case 8:
                printf("Enter the name of the file or directory to delete: ");
                fgets(input, sizeof(input), stdin);
                input[strcspn(input, "\n")] = 0;
                delete_file_or_directory(input);
                break;
            case 9:
                detect_mouse_click();
                break;
            case 10:
                print_log();
                return 0;
            default:
                printf("Unknown choice: %d\n", choice);
        }
    }

    return 0;
}

